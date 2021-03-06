﻿namespace BrightSword.RoslynWrapper

/// <summary>
/// Use this module to specify the syntax for a <code>class</code>
/// </summary>
[<AutoOpen>]
module ClassDeclaration =
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    let private setModifiers modifiers (cd : ClassDeclarationSyntax)  =
        modifiers
        |> Seq.map SyntaxFactory.Token
        |> SyntaxFactory.TokenList
        |> cd.WithModifiers

    let private setMembers members (cd : ClassDeclarationSyntax) =
        cd.AddMembers (members |> Seq.toArray)

    let private setBases bases (cd : ClassDeclarationSyntax) =
        if bases |> Seq.isEmpty then cd else
        bases
        |> Seq.map (ident >> SyntaxFactory.SimpleBaseType >> (fun b -> b :> BaseTypeSyntax))
        |> (SyntaxFactory.SeparatedList >> SyntaxFactory.BaseList)
        |> cd.WithBaseList

    let private setTypeParameters typeParameters (cd : ClassDeclarationSyntax) =
        if typeParameters |> Seq.isEmpty then cd else
        typeParameters
        |> Seq.map (SyntaxFactory.Identifier >> SyntaxFactory.TypeParameter)
        |> (SyntaxFactory.SeparatedList >> SyntaxFactory.TypeParameterList)
        |> cd.WithTypeParameterList

    let ``class`` className ``<<`` typeParameters ``>>``
            ``:`` baseClassName ``,`` baseInterfaces
            modifiers
            ``{``
                 members
            ``}`` =
            className |> (SyntaxFactory.Identifier >> SyntaxFactory.ClassDeclaration)
            |> setTypeParameters typeParameters
            |> setBases (baseClassName ?+ baseInterfaces)
            |> setModifiers modifiers
            |> setMembers members
